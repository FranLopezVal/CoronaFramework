using Corona.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Corona
{
    /// <summary>
    /// Administra las Quest de el videojuego mediante el framework Corona
    /// </summary>
    public class c_QuestSystem : c_System
    {
        bool _use_corona_quest_system = true;
        int _quest_last_id = 0; //Load from save objects TODO!

        public List<c_Quest> availableQuests;
        public List<c_Quest> activeQuests;

        public delegate void QuestEvent(c_Quest quest);

        /// <summary>
        /// On Quest ... se usara para llamar a algun evento que requiera de la 
        /// finalizacion de una mision, o su aceptacion... etc
        /// </summary>
        public event QuestEvent onQuestAccepted;
        public event QuestEvent onQuestCompleted;
        public event QuestEvent onQuestFail;


        public int UniqueId()
        {
            if (_quest_last_id == 0) 
                _quest_last_id = availableQuests.Last().Id;

            int lid = _quest_last_id++;
            return lid; 
        }

        public override void Init(c_SystemEventArg? e)
        {
            _use_corona_quest_system = Corona.Get<bool>("Corona::InventorySystem");
            if (!_use_corona_quest_system) return;

            availableQuests = new List<c_Quest>();
            activeQuests = new List<c_Quest>();
            base.Init(e);
        }

        public override void Update(c_SystemEventArg? e)
        {
            foreach (var q in activeQuests)
            {
                if (!q.IsCompleted)
                    q.OnUpdate();
                else CompleteQuest(q);

            }
        }

        public override bool IsActive()
        {
            return _use_corona_quest_system;
        }
        public void CompleteQuest(c_Quest q)
        {
            if (!_use_corona_quest_system) return;
            if (activeQuests.Contains(q))
            {
                activeQuests.Remove(q);
                //completedQuests.Add(q);
                q.OnComplete();
                onQuestCompleted?.Invoke(q);
            }
        }

        public void MakeActiveQuest(c_Quest q) //Cuando se acepta.
        {
            if (!_use_corona_quest_system) return;
            if (availableQuests.Contains(q))
            {
                activeQuests.Add(q);
                q.OnAccept();
                availableQuests.Remove(q);

                onQuestAccepted?.Invoke(q);
            }
        }

        public void AddAvailableQuest(GameObject go)//Misiones disponibles para aceptar.
        {
            if (!_use_corona_quest_system) return;
            c_Quest q = go.GetComponent<c_Quest>();
            q.Id = UniqueId();
        }
    }
}
