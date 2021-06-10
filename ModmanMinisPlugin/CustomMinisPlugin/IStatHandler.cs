using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LordAshes
{
    public interface IStatHandler
    {
        void CheckStatRequests();
        void SetTransformationRequest(CreatureGuid cid, string content);
        void SyncStealthMode();
        void Reset();
    }
}
