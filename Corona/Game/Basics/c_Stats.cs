using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Corona
{
    /// <summary>
    /// Esta clase contiene los stats de el personaje.
    /// </summary>
    public struct c_StatsContainer
    {


        private List<c_Stats> _stats;

        public c_StatsContainer(List<c_Stats> sts = null)
        {
            if (sts == null)
                _stats = DEFAULT();
            else _stats = sts;
        }

        public c_Stats this[int index]
        {
            get
            {
                return _stats.Find(o => o.Id == index);
            }
            set
            {
                if (_stats.Contains(value))
                {
                    _stats[value.Id] += value;
                }
                else _stats.Add(value);
            }
        }

        public static List<c_Stats> DEFAULT()
        {
            return new List<c_Stats>()
        {
            new c_Stats(c_Stats.HEALT,"HEALT",100),
            new c_Stats(c_Stats.ATTACK,"ATACK",5),
            new c_Stats(c_Stats.ARMOR,"DEFENSE",0),
            new c_Stats(c_Stats.SPEED,"SPEED",0),
            new c_Stats(c_Stats.CRIT_PROB,"CT_PROB",0,true),
            new c_Stats(c_Stats.CRIT_MULT,"CT_MULT",0,true),
            new c_Stats(c_Stats.AT_SPEED,"AT_SPEED",0),
            new c_Stats(c_Stats.STAMINA,"STAMINA",120),
            new c_Stats(c_Stats.STAMINA_RELOAD,"STAMINA_RELOAD",20),
        };
        }
    }

    /// <summary>
    /// Esta clase administra un stat de un 'Character'.
    /// 
    /// </summary>
    public struct c_Stats
    {
        private int _id;
        private string _name;
        private float _value;
        private bool _isMultiplier;

        public c_Stats(int id, string name, float value, bool isMultiplier = false)
        {
            _id = id;
            _name = name;
            _value = value;
            _isMultiplier = isMultiplier;
        }

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public float Value { get => _value; set => _value = value; }
        public bool IsMultiplier { get => _isMultiplier; set => _isMultiplier = value; }

        #region Overloads_Operators

        public static c_Stats operator +(c_Stats a, c_Stats b)
        {
            return new c_Stats(a.Id, a._name, a._value + b.Value, a._isMultiplier);
        }
        public static c_Stats operator -(c_Stats a, c_Stats b)
        {
            return new c_Stats(a.Id, a._name, a._value - b.Value, a._isMultiplier);
        }

        public static bool operator >(c_Stats a, c_Stats b)
        {
            return a.Value > b.Value;
        }
        public static bool operator <(c_Stats a, c_Stats b)
        {
            return a.Value < b.Value;
        }
        public static bool operator >=(c_Stats a, c_Stats b)
        {
            return a.Value >= b.Value;
        }
        public static bool operator <=(c_Stats a, c_Stats b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator ==(c_Stats a, c_Stats b)
        {
            return a.Id == b.Id && a.Name == b.Name;
        }
        public static bool operator !=(c_Stats a, c_Stats b)
        {
            return a.Id != b.Id || a.Name != b.Name;
        }

        #endregion

        #region STATS_ID

        public const int HEALT = 0;
        public const int ATTACK = 1;
        public const int ARMOR = 2;
        public const int SPEED = 3;
        public const int AT_SPEED = 4;
        public const int CRIT_PROB = 5;
        public const int CRIT_MULT = 6;
        public const int STAMINA = 7;
        public const int STAMINA_RELOAD = 8;

        public override bool Equals(object obj)
        {
            return obj is c_Stats stat &&
                   _id == stat._id &&
                   _name == stat._name &&
                   _value == stat._value &&
                   _isMultiplier == stat._isMultiplier;
        }
        // public const int  = 2;
        #endregion
    }
}
