using CompanyApp.Models;

namespace CompanyApp.Repositories
{
    public interface ICompanyRepository
    {
        public Task<Company> GetCompany(int id);
    }
}
