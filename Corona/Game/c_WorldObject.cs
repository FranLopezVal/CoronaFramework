using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Corona
{
    public class c_WorldObject : MonoBehaviour
    {
        protected int _id;

        internal void SetId (int id) { _id = id; }
        public int GetId => _id;

        protected virtual void Interact()
        { }
    }
}
