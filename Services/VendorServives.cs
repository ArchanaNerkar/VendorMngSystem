using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using VendorMngSystem.Dtos;
using VendorMngSystem.IRepositorys;
using VendorMngSystem.IServices;
using VendorMngSystem.Models;
using VendorMngSystem.ViewModel;
using VendorMngSystem.ViewModels;

namespace VendorMngSystem.Services
{
    public class VendorServives:IVendorService
    {
        private readonly IVendorsRepository _vendorsRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public VendorServives(IVendorsRepository vendorsRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _vendorsRepository = vendorsRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<VendorsDto>> GetAllVendors()
        {
            var vendorList = await  _unitOfWork._vendorsRepository.GetVendors().ToListAsync();
            
             return _mapper.Map<List<VendorsDto>>(vendorList);
        }

        public async Task<VendorsDto> GetVendorById(int id)
        {
            var VendorDto = new VendorsDto();

            if (id != 0)
            {
                var vendorRecord = await _vendorsRepository.FindVendorsWithVendorDocument(id);
                if (vendorRecord != null)
                {
                    VendorDto = _mapper.Map<VendorsDto>(vendorRecord);
                }
            }

            VendorDto.vendorCategoriesList = await GetVendorCategoriesDropDown();
           return VendorDto;
        }

        public async Task<(int rowChanged, int vendorId)> CreateUpdateVendor(VendorsDto dto, string webRootPath)
        {
            var filesToDeleteFromDisk = new List<string>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var vendormodel=new Vendors();

                if (dto.VendorId == 0)
                {
                    vendormodel = _mapper.Map<Vendors>(dto);
                    vendormodel.VendorsCategoryId = dto.VendorsCategoryId;
                    vendormodel.vendorsCategories = null;

                    // Ensure navigation collection is not null
                   // vendormodel.vendorDocuments ??= new List<VendorDocuments>();
                }
                else
                {
                    var existingRecord = await _unitOfWork._vendorsRepository.FindVendorsWithVendorDocument(dto.VendorId);
                    if (existingRecord == null)
                    {
                        await _unitOfWork.RollBack();
                        return (0, dto.VendorId);
                    }

                    // Map scalar values onto tracked entity
                    _mapper.Map(dto, existingRecord);
                    vendormodel = existingRecord;

                    vendormodel.VendorsCategoryId = dto.VendorsCategoryId;
                    vendormodel.vendorsCategories = null;

                   
                    var keptIds = dto.KeptDocumentIds ?? new List<int>();

                    var docsToRemove = vendormodel.vendorDocuments
                        .Where(d => !keptIds.Contains(d.VendorsDocumentId))
                        .ToList();

                    foreach (var doc in docsToRemove)
                    {
                        if (!string.IsNullOrEmpty(doc.VendorFilePath))
                        {
                            string physicalPath = Path.Combine(webRootPath, doc.VendorFilePath.TrimStart('/', '\\'));
                            filesToDeleteFromDisk.Add(physicalPath);
                        }

                        _unitOfWork._vendorDocRepository.DeleteVendorsDcot(doc);
                    }
                }

                if (dto.VendorDocumentsPath != null && dto.VendorDocumentsPath.Count > 0)
                {
                    string uploadsFolder = Path.Combine(webRootPath, "uploadsDocumnets", "VendorDocuments");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in dto.VendorDocumentsPath)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var docDto = new VendorDocumentsDto
                            {
                                VendorFileName = file.FileName,
                                VendorFilePath = $"/uploadsDocumnets/VendorDocuments/{uniqueFileName}"
                            };

                            var vendorDoc = _mapper.Map<VendorDocuments>(docDto);
                            vendormodel.vendorDocuments.Add(vendorDoc);
                        }
                    }
                }

                if (dto.VendorId == 0)
                {
                    await _unitOfWork._vendorsRepository.CreateVendor(vendormodel);
                }
                else
                {
                    _unitOfWork._vendorsRepository.UpdateVendor(vendormodel);
                }

                int rowChanged = await _unitOfWork.CommitTransaction();

                foreach (var filePath in filesToDeleteFromDisk)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                return (rowChanged, vendormodel.VendorId);
            }
            catch
            {
                // Rollback DB in transi
                //await _unitOfWork.RollBack();
                throw;
            }
        }
        public async Task<int> DeleteVendorRecord(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
               var vendor = await _unitOfWork._vendorsRepository.FindVendorsById(id);
                if (vendor == null)
                {
                    return 0;
                }
                _unitOfWork._vendorsRepository.DeleteVendorsDeatails(vendor);

                int rowsChanged = await _unitOfWork.CommitTransaction();
                return rowsChanged;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBack();
                return 0;
            }
        }



        public async Task<object> GetVendorsDataTable(DatatableParameters parameters)
        {
            var Record = _unitOfWork._vendorsRepository.GetVendors();
            int totalRecords = Record.Count();
            if (!string.IsNullOrEmpty(parameters.search?.value))
            {
                var search = parameters.search.value;
                Record = Record.Where(e =>
                    e.VendorName.Contains(search) || e.Emails.Contains(search) ||
                    e.GSTNumber.Contains(search));
            }
            int filteredRecords = Record.Count();

            if (parameters.order?.Count > 0)
            {
                var col = parameters.columns[parameters.order[0].column].name;
                var dir = parameters.order[0].dir;
                Record = Record.OrderBy($"{col} {dir}"); //
            }
            else
            {
                Record = Record.OrderBy(e => e.VendorId);
            }

            //var data = Record
            //    .Skip(parameters.start)
            //    .Take(parameters.length).Select(c => new { c.VendorId, c.VendorName, c.Emails, c.ContactPerson,c.MobileNumbers,
            //     c.GSTNumber,c.VendorsCategoryId,c.VendorStatus})
            //    .ToList();
            var data = Record
               .Skip(parameters.start)
               .Take(parameters.length).Select(c => new {
                   c.VendorId,
                   c.VendorName,
                   c.Emails,
                   c.ContactPerson,
                   c.MobileNumbers,
                   c.GSTNumber,
                   //c.vendorsCategories?.VendorsCategoryName ?? "No Assign",
                   VendorsCategoryName = c.vendorsCategories != null ? c.vendorsCategories.VendorsCategoryName : "No Assign",//c.VendorsCategoryId,
                   status = c.VendorStatus == true ? "Active" : "No Active"
               })
               .ToList();
            return new
            {
                draw = parameters.draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = data
            };
        }

        public async Task<List<SelectListItem>> GetVendorCategoriesDropDown()
        {
            var Dropdown = await _unitOfWork._vendorsRepository.VendorCategoriesDropDown();

            return Dropdown.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.VendorsCategoryName
            }).ToList();
        }



        public async Task<List<VendorDocumentsDto>> VendorDocumentsList(int vendorId)
        {

            return  await _unitOfWork._vendorsRepository.VendorDocumentsList(vendorId);
            

        
        }

    }
}

