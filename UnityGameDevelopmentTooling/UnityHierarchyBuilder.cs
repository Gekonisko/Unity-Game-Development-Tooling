using System.Text;
using Unity_Game_Development_tooling.Models;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;

namespace UnityGameDevelopmentTooling
{
    public class UnityHierarchyBuilder : IHierarchyBuilder
    {
        private readonly Dictionary<long, UnityObject> _objects;
        private StringBuilder _buffer;

        public UnityHierarchyBuilder(Dictionary<UnityObjectInfo, UnityObject> objects)
        {
            _objects = objects.ToDictionary(
                kvp => kvp.Key.FileId,
                kvp => kvp.Value
            );
        }

        public string Build()
        {
            _buffer = new StringBuilder();

            foreach (var root in GetRootNodes())
            {
                _buffer.AppendLine(root.Name);
                AppendChildren(root, 1);
            }

            return _buffer.ToString();
        }

        private void AppendChildren(GameObject parent, int depth)
        {
            var transform = GetTransformFromGameObject(parent);
            if (transform == null)
                return;

            foreach (var childId in transform.Children)
            {
                if (_objects.TryGetValue(childId.fileID, out var unityObject) &&
                    unityObject is Transform child)
                {
                    var childGameObject = GetGameObjectFromTransform(child);
                    if (childGameObject != null)
                    {
                        _buffer.AppendLine($"{new string('-', depth * 2)}{childGameObject.Name}");
                        AppendChildren(childGameObject, depth + 1);
                    }
                }
            }
        }

        private List<GameObject> GetRootNodes()
        {
            List<GameObject> roots = new();
            foreach (var kvp in _objects)
            {
                if (kvp.Value is GameObject == false) continue;
                var go = (GameObject)kvp.Value;

                if (GetTransformFromGameObject(go, kvp.Key)?.Father.fileID == 0)
                {
                    roots.Add(go);
                }
            }
            return roots;
        }

        private Transform? GetTransformFromGameObject(GameObject gameObject, long id = 0)
        {
            foreach (ComponentRef component in gameObject.Components)
            {
                if (_objects.TryGetValue(component.component.fileID, out var value) && value is Transform)
                {
                    return (Transform)value;
                }
            }
            return null;
        }

        private GameObject? GetGameObjectFromTransform(Transform transform)
        {
            if (_objects.TryGetValue(transform.GameObject.fileID, out UnityObject value) && value is GameObject)
            {
                return (GameObject)value;
            }
            return null;
        }
    }
}