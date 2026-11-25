using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Asigna campos [SerializeField] privados desde los builders del editor.</summary>
    internal static class SerializedField
    {
        public static void Set(Object target, string fieldName, Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogError($"[Clous-End] No existe el campo '{fieldName}' en {target.GetType().Name}.");
                return;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, float value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, int value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, bool value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.boolValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, string value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, Color value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.colorValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, Vector2 value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.vector2Value = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void Set(Object target, string fieldName, Vector3 value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);
            if (property == null) return;

            property.vector3Value = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
