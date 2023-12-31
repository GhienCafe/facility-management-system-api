﻿using API_FFMS.Dtos;
using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IInventoryCheckRepository
{
    Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Room?> rooms, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> ConfirmOrReject(InventoryCheck inventoryCheck, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null);
    Task<bool> UpdateInventory(InventoryCheck inventoryCheck,
                                    List<Report?> additionMediaFiles,
                                    List<Report?> removalMediaFiles,
                                    List<InventoryCheckDetail?> additionInventoryDetails,
                                    List<InventoryCheckDetail?> removalInventoryDetails,
                                    Guid? editorId, DateTime? now = null);
}

public class InventoryCheckRepository : IInventoryCheckRepository
{
    private readonly DatabaseContext _context;

    public InventoryCheckRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Room?> rooms, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.Id = Guid.NewGuid();
            inventoryCheck.CreatedAt = now.Value;
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.CreatorId = creatorId;
            inventoryCheck.Status = RequestStatus.NotStart;
            inventoryCheck.RequestDate = now.Value;
            await _context.InventoryChecks.AddAsync(inventoryCheck);

            foreach (var room in rooms)
            {
                if (room != null)
                {
                    var assets = _context.RoomAssets
                                             .Where(ra => ra.RoomId == room.Id && ra.ToDate == null)
                                             .Select(ra => ra.Asset)
                                             .ToList();

                    foreach (var asset in assets)
                    {
                        if (asset != null)
                        {
                            var roomAsset = _context.RoomAssets
                                                .Where(ra => ra.RoomId == room.Id && ra.AssetId == asset.Id && ra.ToDate == null)
                                                .FirstOrDefault();
                            var inventoryCheckDetail = new InventoryCheckDetail
                            {
                                Id = Guid.NewGuid(),
                                AssetId = asset.Id,
                                InventoryCheckId = inventoryCheck.Id,
                                CreatorId = creatorId,
                                CreatedAt = now.Value,
                                RoomId = room.Id,
                                StatusBefore = asset.Status,
                                QuantityBefore = roomAsset!.Quantity
                            };

                            await _context.InventoryCheckDetails.AddAsync(inventoryCheckDetail);
                        }
                    }
                }
            }

            if (mediaFiles != null)
            {
                foreach (var mediaFile in mediaFiles)
                {
                    var newMediaFile = new Report
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = now.Value,
                        CreatorId = creatorId,
                        EditedAt = now.Value,
                        EditorId = creatorId,
                        FileName = mediaFile.FileName,
                        Uri = mediaFile.Uri,
                        FileType = FileType.File,
                        ItemId = inventoryCheck.Id
                    };
                    _context.MediaFiles.Add(newMediaFile);
                }
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = inventoryCheck.Description ?? "Yêu cầu đi kiểm kê",
                Title = RequestType.InventoryCheck.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = inventoryCheck.Id,
                UserId = inventoryCheck.AssignedTo
            };
            await _context.Notifications.AddAsync(notification);

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> ConfirmOrReject(InventoryCheck inventoryCheck, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.EditorId = editorId;
            _context.Entry(inventoryCheck).State = EntityState.Modified;

            var inventoryCheckDetails = _context.InventoryCheckDetails
                                            .Include(x => x.Asset)
                                            .Where(x => x.InventoryCheckId == inventoryCheck.Id)
                                            .ToList();

            var reports = await _context.MediaFiles.FirstOrDefaultAsync(x => x.ItemId == inventoryCheck.Id && !x.IsReject && x.IsReported);

            if (confirmOrRejectDto?.Status == RequestStatus.Done)
            {
                foreach (var detail in inventoryCheckDetails)
                {
                    var roomAsset = _context.RoomAssets
                                .FirstOrDefault(x => x.AssetId == detail.AssetId && x.RoomId == detail.RoomId);
                    var asset = _context.Assets.Include(x => x.Type)
                            .FirstOrDefault(x => x.Id == detail.AssetId);

                    if (asset != null && asset.Type!.Unit == Unit.Individual)
                    {
                        asset.Status = detail.StatusReported;
                        _context.Entry(asset).State = EntityState.Modified;

                        if (roomAsset != null)
                        {
                            roomAsset.Quantity = 1;
                            roomAsset.Status = detail.StatusReported;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }
                    }
                    else if (asset != null && asset.Type!.Unit == Unit.Quantity)
                    {
                        if (roomAsset != null)
                        {
                            roomAsset.Quantity = detail.QuantityReported;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }
                    }
                }

                inventoryCheck.CompletionDate = now.Value;
                inventoryCheck.Status = RequestStatus.Done;
                _context.Entry(inventoryCheck).State = EntityState.Modified;

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = "Đã xác nhận",
                    Title = "Đã xác nhận",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
                    IsRead = false,
                    ItemId = inventoryCheck.Id,
                    UserId = inventoryCheck.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);
            }
            else if (confirmOrRejectDto?.Status == RequestStatus.NeedAdditional)
            {
                inventoryCheck.Status = RequestStatus.InProgress;
                _context.Entry(inventoryCheck).State = EntityState.Modified;

                if (reports != null)
                {
                    reports.IsReject = true;
                    reports.RejectReason = confirmOrRejectDto.Reason;
                    _context.Entry(reports).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = confirmOrRejectDto.Reason ?? "Cần bổ sung",
                    Title = "Báo cáo lại",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
                    IsRead = false,
                    ItemId = inventoryCheck.Id,
                    UserId = inventoryCheck.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);
            }
            else if (confirmOrRejectDto?.Status == RequestStatus.Cancelled)
            {
                inventoryCheck.Status = RequestStatus.Cancelled;
                _context.Entry(inventoryCheck).State = EntityState.Modified;

                if (reports != null)
                {
                    reports.IsReject = true;
                    reports.RejectReason = confirmOrRejectDto.Reason;
                    _context.Entry(reports).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = confirmOrRejectDto.Reason ?? "Hủy yêu cầu",
                    Title = "Hủy yêu cầu",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
                    IsRead = false,
                    ItemId = inventoryCheck.Id,
                    UserId = inventoryCheck.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> UpdateInventory(InventoryCheck inventoryCheck,
                                                 List<Report?> additionMediaFiles,
                                                 List<Report?> removalMediaFiles,
                                                 List<InventoryCheckDetail?> additionInventoryDetails,
                                                 List<InventoryCheckDetail?> removalInventoryDetails,
                                                 Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.EditorId = editorId;
            inventoryCheck.EditedAt = now.Value;
            _context.Entry(inventoryCheck).State = EntityState.Modified;

            var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                .Where(x => x.ItemId == inventoryCheck.Id && !x.DeletedAt.HasValue)
                                                .ToList();

            var inventoryDetails = _context.InventoryCheckDetails.AsNoTracking()
                                                .Where(x => x.InventoryCheckId == inventoryCheck.Id && !x.DeletedAt.HasValue)
                                                .ToList();

            if(additionInventoryDetails.Count > 0)
            {
                foreach(var item in additionInventoryDetails)
                {
                    if(item != null)
                    {
                        _context.InventoryCheckDetails.Add(item);
                    }
                }
            }

            if (removalInventoryDetails.Count > 0)
            {
                foreach (var item in removalInventoryDetails)
                {
                    if (item != null)
                    {
                        _context.InventoryCheckDetails.Remove(item);
                    }
                }
            }

            if (additionMediaFiles.Count > 0)
            {
                foreach (var item in additionMediaFiles)
                {
                    if (item != null)
                    {
                        _context.MediaFiles.Add(item);
                    }
                }
            }

            if (removalMediaFiles.Count > 0)
            {
                foreach (var item in removalMediaFiles)
                {
                    if (item != null)
                    {
                        _context.MediaFiles.Remove(item);
                    }
                }
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }
}