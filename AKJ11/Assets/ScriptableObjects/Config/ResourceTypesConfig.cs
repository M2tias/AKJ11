using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ResourceTypesConfig", menuName = "Configs/New ResourceTypesConfig")]
public class ResourceTypesConfig : ScriptableObject
{

    [field: SerializeField]
    public List<ResourceTypeConfig> Types {get; private set;}

    public ResourceTypeConfig Get(ResourceType type) {
        return Types.Where(rType => rType.Type == type).FirstOrDefault();
    }

}
