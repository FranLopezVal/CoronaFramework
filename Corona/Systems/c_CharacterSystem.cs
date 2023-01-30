
using Corona.Systems;
using System;
using UnityEngine;

namespace Corona
{
    public delegate void BuffCallBackUpdate();
    public class c_CharacterSystem : c_System
    {
        private int[,] _racesRelationship;
        public c_CharacterSystem()
        {

        }

        public override void Init(c_SystemEventArg? e)
        {
            _racesRelationship = new int[,]
            {   //H - E - D - O - ND
                {-1 ,0  ,0  ,0  ,0  },//HUMANS
                {0  ,-1 ,0  ,0  ,0  },//ELFS
                {0  ,0  ,-1 ,0  ,0  },//DWARFS
                {0  ,0  ,0  ,-1 ,0  },//ORCS
                {0  ,0  ,0  ,0  ,-1 }//NODEATHS
            };
            
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _racesRelationship[i, j]= (i==j)? -1 : 0;
                }
            }
        }

        public override void Update(c_SystemEventArg? e)
        {
            base.Update(e);
        }
        /// <summary>
        /// Cambia La relacion entre dos razas.
        /// </summary>
        /// <param name="ra"></param>
        /// <param name="rb"></param>
        /// <param name="rel"></param>
        public void ChangeRaceRelation(Race ra,Race rb,int rel)
        {
            _racesRelationship[(int)ra, (int)rb] = rel;
            _racesRelationship[(int)rb, (int)ra] = rel;
        }

        public bool CharactersInConflicts(c_Character a, c_Character b)
        {
            int a_ = (int)a.Race.GetRace;
            int b_ = (int)b.Race.GetRace;
            int r = _racesRelationship[a_, b_];
            
            return (r == 3);
        }


    }
}
