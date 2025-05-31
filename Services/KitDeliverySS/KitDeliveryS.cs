﻿using Repositories.Pagging;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.KitDeliveryRepo;
using Model;

namespace Services.KitDeliverySS
{
    public class KitDeliveryS : IKitDeliveryS
    {

        private readonly IKitDeliveryRepository _kitDeliveryRepository;
        public KitDeliveryS(IKitDeliveryRepository kitDeliveryRepository)
        {
            _kitDeliveryRepository = kitDeliveryRepository;
        }
        public async Task<KitDelivery> AddAsync(AddKitDeliveryDTO addKitDeliveryDto)
        {
            if (addKitDeliveryDto == null)
            {
                throw new ArgumentNullException(nameof(addKitDeliveryDto), "Kit delivery data is required.");
            }

            if (addKitDeliveryDto.ReceivedAt <= addKitDeliveryDto.SentAt)
            {
                throw new ArgumentException("ReceivedAt must be after SentAt.", nameof(addKitDeliveryDto.SentAt));
            }

            var delivery = new KitDelivery
            {
                RequestId = addKitDeliveryDto.RequestId,
                KitId = addKitDeliveryDto.KitId,
                SentAt = addKitDeliveryDto.SentAt.Kind == DateTimeKind.Unspecified
         ? DateTime.SpecifyKind(addKitDeliveryDto.SentAt, DateTimeKind.Utc)
         : addKitDeliveryDto.SentAt.ToUniversalTime(),
                ReceivedAt = addKitDeliveryDto.ReceivedAt.Kind == DateTimeKind.Unspecified
         ? DateTime.SpecifyKind(addKitDeliveryDto.ReceivedAt, DateTimeKind.Utc)
         : addKitDeliveryDto.ReceivedAt.ToUniversalTime(),
                StatusId = addKitDeliveryDto.StatusId
            };

            await _kitDeliveryRepository.AddAsync(delivery);
            return delivery;
        }

        public async Task DeleteAsync(int id)
        {
            var delivery = await _kitDeliveryRepository.GetByIdAsync(id);
            if (delivery == null)
            {
                throw new KeyNotFoundException($"KitDelivery with ID {id} not found.");
            }
            await _kitDeliveryRepository.DeleteAsync(id);

        }

        public async Task<PaginatedList<KitDelivery>> GetAll(int pageNumber, int pageSize)
        {
            IQueryable<KitDelivery> kitDeliveries = _kitDeliveryRepository.GetAll().AsQueryable();
            return await PaginatedList<KitDelivery>.CreateAsync(kitDeliveries, pageNumber, pageSize);
        }

        public async Task<IEnumerable<KitDelivery>> GetAllKitAsync()
        {
            return await _kitDeliveryRepository.GetkitAsync();
        }

        public async Task<KitDelivery?> GetByIdAsync(int id)
        {
            return await _kitDeliveryRepository.GetByIdAsync(id);
        }

        public async Task<KitDelivery?> UpdateAsync(int id, UpdateKitDeliveryDTO updateKitDeliveryDto)
        {

            if (updateKitDeliveryDto == null)
            {
                throw new ArgumentNullException(nameof(updateKitDeliveryDto), "kitDeliveries data is required.");
            }

            var kitDeliveries = await _kitDeliveryRepository.GetByIdAsync(id);
            if (kitDeliveries == null)
            {
                throw new KeyNotFoundException($"kitDeliveries with ID {id} not found.");
            }
            kitDeliveries.ReceivedAt = updateKitDeliveryDto.ReceivedAt.Kind == DateTimeKind.Unspecified
       ? DateTime.SpecifyKind(updateKitDeliveryDto.ReceivedAt, DateTimeKind.Utc)
       : updateKitDeliveryDto.ReceivedAt.ToUniversalTime();
            kitDeliveries.StatusId = updateKitDeliveryDto.StatusId;

            await _kitDeliveryRepository.UpdateAsync(kitDeliveries);

            return kitDeliveries;

        }
    }
}


