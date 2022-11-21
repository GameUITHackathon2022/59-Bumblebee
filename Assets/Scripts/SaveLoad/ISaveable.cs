namespace SaveLoad {
    using System;

    public interface ISaveable {
        void Save(string saveKey);
        void Load(string loadKey);
    }
}

