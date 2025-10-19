using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity_Game_Development_tooling
{
    public class UnitySceneSerializer
    {
        public static string Deserialize(string path)
        {
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                }
            }

                return "";
        }
    }
}
