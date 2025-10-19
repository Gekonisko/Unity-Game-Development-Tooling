using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityGameDevelopmentTooling.Interfaces
{
    public interface IYamlDeserializer
    {
        object Deserialize(string yaml, Type type);

        Dictionary<string, object> DeserializeHeader(string yamlHeader);
    }
}