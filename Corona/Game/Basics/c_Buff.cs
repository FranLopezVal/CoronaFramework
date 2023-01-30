using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Corona
{
    public class c_Buff
    {
        protected string _name;
        protected string _definition;

        protected int _id;

        protected c_Character _instantiator;
        protected c_Character _target;

        protected bool _disposed = false;
        protected float _duration;


        private float _timeStartBuff;
        private float _currentTimeStep;

        public float BuffTimeStep => _currentTimeStep;
        public float BuffDuration => _duration;
        public bool Disposed => _disposed;

        public int GetId => _id;
        public string GetName => _name;
        public string GetDefinition => _definition;

        public c_Buff(c_Character instantiator)
        {
            _instantiator = instantiator;
        }

        public virtual void StartBuff(c_Character target)
        {
            _target = target;
            _timeStartBuff = Time.time;
            UpdateBuff();
        }

        public virtual void UpdateBuff()
        {
            if (_disposed) return;
            _currentTimeStep += Time.deltaTime;

            if (Time.time > _duration + _timeStartBuff) EndBuff();

        }

        public void AddDuration(c_Buff c)
        {
            _duration += c._duration;
        }
        public void AddDuration(int timeToAadd)
        {
            _duration += timeToAadd;
        }

        public void RemoveBuff() { EndBuff(); }
        protected virtual void EndBuff()
        {
            _disposed = true;
            _instantiator = null;
        }

        public override bool Equals(object? obj)
        {
            if(obj.GetType() is c_Buff)
            {
                return (((c_Buff)obj).GetId == this.GetId) ;
            }
            else return (((c_Buff)obj).GetId == this.GetId);
           // return false;
        }
        public static bool operator ==(c_Buff c1, c_Buff c2)
        {
            return c1._id == c2._id;
        }
        public static bool operator !=(c_Buff c1, c_Buff c2)
        {
            return c1._id != c2._id;
        }
    }
}
