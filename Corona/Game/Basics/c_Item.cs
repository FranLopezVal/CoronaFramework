using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Corona
{
    [System.Serializable]
    public class c_Item
    {
        private string name;
        private int id;
        private string description_text_ref;
        private Sprite icon;
        private int amount;
        private int maxStack;
        private bool isUnique;
        private bool isUsable;

        #region Encapsulations
        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }
        public Sprite Icon { get => icon; set => icon = value; }
        public int Amount { get => amount; set => amount = value; }
        public int MaxStack { get => maxStack; set => maxStack = value; }
        public bool IsUnique { get => isUnique; set => isUnique = value; }
        public bool IsUsable { get => isUsable; set => isUsable = value; }
        public string Description_text_ref { get => description_text_ref; set => description_text_ref = value; }

        #endregion
    }
}
