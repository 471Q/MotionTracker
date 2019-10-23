using FireSharp.Config;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainScreenUI
{
    class FireS
    {
        public IFirebaseConfig ifc;
        public IFirebaseClient client;
        public void SetIFC()
        {
            ifc = new FirebaseConfig()
            {
                AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
                BasePath = "https://motiontracker-dd816.firebaseio.com/"
            };
        }

        public void SetClient()
        {
            client = new FireSharp.FirebaseClient(ifc);
        }
    }
}
