using Unity.Entities;
using UnityEngine;

namespace GameOfLife
{
    public class GameManager : SystemBase
    {
        private bool _isSimulationOn;

        public bool IsSimulationOn
        {
            get => _isSimulationOn;
            
            set
            {
                _isSimulationOn = value;
                
                if (value)
                {
                    if (TryGetSingletonEntity<EditModeTag>(out var editModeTag))
                    {
                        EntityManager.DestroyEntity(editModeTag);
                    }

                    EntityManager.CreateEntity(typeof(SimulationModeTag));
                }
                else
                {
                    if (TryGetSingletonEntity<SimulationModeTag>(out var simulationModeTag))
                    {
                        EntityManager.DestroyEntity(simulationModeTag);
                    }

                    EntityManager.CreateEntity(typeof(EditModeTag));
                }
            }
        }

        protected override void OnCreate()
        {
            Enabled = false;
            IsSimulationOn = false;
        }

        protected override void OnUpdate()
        {
            Debug.Log("Game manager updated");
        }
    }
}