using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity_Game_Development_tooling.Models;

namespace UnityGameDevelopmentTooling.Interfaces
{
    public interface ISceneParser
    {
        IEnumerable<(UnityObjectInfo Header, string Yaml)> Parse(string path);
    }
}