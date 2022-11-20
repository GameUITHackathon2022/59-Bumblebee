namespace Dialogue {
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue", order = 0)]
    public class DialogueScriptableObject : JSONSerializableScriptableObject<DialogueScriptableObject> {
        public string key;
        public TextObject[] texts;

        private static HashSet<string> _randomStrings = new HashSet<string>();

        public TextObject GetTextObject(int index) {
            if (index < Length && index >= 0) return texts[index];
            return null;
        }

        public int Length => texts.Length;
    }
}


