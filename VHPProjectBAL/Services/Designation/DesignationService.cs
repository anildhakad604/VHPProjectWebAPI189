using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.Repository.DesignationRepo;
using VHPProjectDTOModel.DesignationDTO.request;
using VHPProjectDTOModel.DesignationDTO.response;

namespace VHPProjectBAL.Services.Designation
{
    public class DesignationService:IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;

        public DesignationService(IDesignationRepository designationRepository)
        {
            _designationRepository = designationRepository;
        }

        public async Task<DesignationListResponse_DTO> GetActiveDesignationsAsync()
        {
            var entities = await _designationRepository.GetActiveDesignationAsync();
            var list = entities.Select(d => new DesignationResponse_DTO
            {
                DesignationId = d.DesignationId,
                DesignationName = d.DesignationName,
                IsActive = d.IsActive
            }).ToList();

            return new DesignationListResponse_DTO { Designations = list };
        }

        //public async Task<DesignationResponse_DTO?> GetByIdAsync(int id)
        //{
        //    var entity = await _designationRepository.GetByIdAsync(id);
        //    if (entity == null) return null;

        //    return new DesignationResponse_DTO
        //    {
        //        DesignationId = entity.DesignationId,
        //        DesignationName = entity.DesignationName,
        //        IsActive = entity.IsActive
        //    };
        //}

        //public async Task AddDesignationAsync(AddDesignationRequest_DTO request)
        //{
        //    //var entity = new Designation
        //    //{
        //    //    DesignationName = request.DesignationName,
        //    //    IsActive = true
        //    //};

        //    //await _designationRepository.AddAsync(entity);
        //}

        //public async Task UpdateDesignationAsync(UpdateDesignationRequest_DTO request)
        //{
        //    var entity = await _designationRepository.GetByIdAsync(request.DesignationId);
        //    if (entity == null) throw new Exception("Designation not found.");

        //    entity.DesignationName = request.DesignationName;
        //    entity.IsActive = request.IsActive;

        //    await _designationRepository.UpdateAsync(entity);
        //}

        //public async Task DeleteDesignationAsync(int id)
        //{
        //    await _designationRepository.DeleteAsync(id);
        //}
    }
}
