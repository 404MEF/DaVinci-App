using System.Threading.Tasks;

namespace Davinci
{
    public static class Token
    {
        public static string value;
        public static async Task<bool> VerifyToken()
        {
            return (await Api.DavinciApi.VerifyToken(value)).OK;
        }
    }
}