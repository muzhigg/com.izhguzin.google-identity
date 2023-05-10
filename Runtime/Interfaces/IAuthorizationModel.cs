using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    internal interface IAuthorizationModel
    {
        public Task Authorize();
    }
}