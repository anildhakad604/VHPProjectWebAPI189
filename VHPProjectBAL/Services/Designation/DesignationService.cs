    using AutoMapper;
    using VHPProjectCommonUtility.Logger;
    using VHPProjectCommonUtility.Response;
    using VHPProjectDAL.DesignationRepo;
    using VHPProjectDTOModel.DesignationDTO.request;
    using VHPProjectDTOModel.DesignationDTO.response;

    namespace VHPProjectBAL.Services.Designation
    {
        public class DesignationService : IDesignationService
        {
            private readonly IDesignationRepository _designationRepository;
            private readonly IMapper _mapper;
            private readonly ILoggerManager _logger;

            public DesignationService(IDesignationRepository designationRepository,IMapper mapper,ILoggerManager logger)
            {
                _designationRepository = designationRepository;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<ResultWithDataDTO<IEnumerable<DesignationResponse_DTO>>> GetActiveAsync()
            {
                var response = new ResultWithDataDTO<IEnumerable<DesignationResponse_DTO>>();

                try
                {
                    _logger.LogInfo("Fetching active designations...");

                    var data = await _designationRepository.GetActiveDesignationAsync();
                    var mapped = _mapper.Map<IEnumerable<DesignationResponse_DTO>>(data);

                    response.IsSuccessful = true;
                    response.Message = "Active designations fetched successfully";
                    response.Data = mapped;

                    _logger.LogInfo("Successfully fetched active designations.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in GetActiveAsync(): {ex.Message}");

                    response.IsSuccessful = false;
                    response.SystemErrorMessage = "Unable to fetch designations. Please contact admin.";
                    response.Data = null;
                }

                return response;
            }


       

            public async Task<ResultWithDataDTO<DesignationResponse_DTO?>> GetByIdAsync(int id)
            {
                var result = new ResultWithDataDTO<DesignationResponse_DTO?>();

                try
                {
                    var entity = await _designationRepository.GetByIdAsync(id);

                    if (entity == null)
                    {
                        result.IsSuccessful = false;
                        result.IsBusinessError = true;
                        result.BusinessErrorMessage = "Designation not found.";
                        result.Message = "Designation not found.";
                    }
                    else
                    {
                        var dto = _mapper.Map<DesignationResponse_DTO>(entity);

                        result.IsSuccessful = true;
                        result.Message = "Designation retrieved successfully.";
                        result.Data = dto;
                    }

                    result.CurrentPage = 1;
                    result.PageSize = 1;
                    result.TotalCount = entity == null ? 0 : 1;
                    result.TotalPages = 1;
                
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    result.IsSuccessful = false;
                    result.IsSystemError = true;
                    result.SystemErrorMessage = ex.Message;
                    result.Message = "Error retrieving designation.";
                }

                return result;
            }


        
            public async Task<ResultWithDataDTO<string>> UpdateDesignationAsync(UpdateDesignationRequest_DTO request)
            {
                var result = new ResultWithDataDTO<string>();

                try
                {
                    var entity = _mapper.Map<VHPProjectDAL.DataModel.Designation>(request);

                    await _designationRepository.UpdateAsync(entity);

                    result.IsSuccessful = true;
                    result.Message = "Designation updated successfully.";
                    result.Data = "Updated";

                    result.CurrentPage = 1;
                    result.PageSize = 1;
                    result.TotalCount = 1;
                    result.TotalPages = 1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    result.IsSuccessful = false;
                    result.IsSystemError = true;
                    result.SystemErrorMessage = ex.Message;
                    result.Message = "Error updating designation.";
                }

                return result;
            }



            public async Task<ResultWithDataDTO<string>> DeleteDesignationAsync(int id)
            {
                var result = new ResultWithDataDTO<string>();

                try
                {
                    await _designationRepository.DeleteAsync(id);

                    result.IsSuccessful = true;
                    result.Message = "Designation deleted successfully.";
                    result.Data = "Deleted";

                    result.CurrentPage = 1;
                    result.PageSize = 1;
                    result.TotalCount = 1;
                    result.TotalPages = 1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    result.IsSuccessful = false;
                    result.IsSystemError = true;
                    result.SystemErrorMessage = ex.Message;
                    result.Message = "Error deleting designation.";
                }

                return result;
            }





        

            public async Task<ResultWithDataDTO<string>> AddDesignationAsync(AddDesignationRequest_DTO request)
            {
                var result = new ResultWithDataDTO<string>();

                try
                {
                    var entity = _mapper.Map<VHPProjectDAL.DataModel.Designation>(request);

                    await _designationRepository.AddAsync(entity);

                    result.IsSuccessful = true;
                    result.Message = "Designation added successfully.";
                    result.Data = "Success";

                    result.CurrentPage = 1;
                    result.PageSize = 1;
                    result.TotalCount = 1;
                    result.TotalPages = 1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    result.IsSuccessful = false;
                    result.IsSystemError = true;
                    result.SystemErrorMessage = ex.Message;
                    result.Message = "Error adding designation.";
                }

                return result;
            }
        }
    }
