using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprado.Enums
{
    internal class DatabaseMethodResponseEnum
    {

        public enum DatabaseResponse
        {
            OK,
            CREATED,
            REMOVED,
            EDITED,
            ERROR,
            BAD_VERIFICATION,
            BAD_INPUT
        }

    }
}
