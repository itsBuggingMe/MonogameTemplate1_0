using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTemplate1_0
{

    /// <summary>
    /// A quick and dirty FSM class
    /// </summary>
    public class StateMachine
    {
        private static StateMachine _instance;
        private IState _currentState;

        public List<IState> PreviousStates = new List<IState>();

        /// <summary>
        /// The singleton instance of the StateMachine
        /// </summary>
        public static StateMachine Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("State has not been intialised. Call StateMachine.Initalise");
                }
                return _instance;
            }
        }

        private StateMachine(IState initalState)
        {
            _currentState = initalState;
        }

        /// <summary>
        /// Initalises the State Machine. Please call only once
        /// </summary>
        public static void Initalise(IState initalState)
        {
            _instance = new StateMachine(initalState);
        }

        /// <summary>
        /// Changes State to the new state, and passes on the information
        /// </summary>
        public void ChangeState(IState newState)
        {
            PreviousStates.Remove(_currentState);

            PreviousStates.Add(_currentState);

            newState.OnStateEnter(_currentState.OnStateExit());
            _currentState = newState;
        }

        /// <summary>
        /// Ticks the state machine 1 = 1/60 of a second, so a deltaTime of 15 means 0.25 seconds have passed
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Globals.Inst.Game.Exit();

            _currentState.Tick(deltaTime);
        }

        /// <summary>
        /// Draws the current State
        /// </summary>
        public void Draw()
        {
            Globals.Inst.spriteBatch.Begin();
            Globals.Inst.shapeBatch.Begin();

            _currentState.Draw();
            DebugDrawer.Debug.Draw();

            Globals.Inst.spriteBatch.End();
            Globals.Inst.shapeBatch.End();

            Globals.Inst.postShapeBatch.Begin();
            Globals.Inst.postShapeBatch.End();

            Globals.Inst.drawBuffer.Draw();
        }
    }

    /// <summary>
    /// Basic of all states in the State Machine class
    /// </summary>
    public interface IState
    {
        public List<GameSystem> Systems { get; }
        public List<Entity> Entities { get; }

        /// <summary>
        /// 1 = 1/60 of a second, so a deltaTime of 15 means 0.25 seconds have passed
        /// </summary>
        public void Tick(float deltaTime);

        /// <summary>
        /// Draws the State
        /// </summary>
        public void Draw();

        /// <summary>
        /// Called whenever the state is entered. Previous State messages is whatever the previous state deicides to send. the type of the previous state can be acsessed through StateMachine.Instance.PreviousStates
        /// </summary>
        public void OnStateEnter(List<string> previousStateMessages);

        /// <summary>
        /// Called whenever the state is entered. Return arguments nessecary for the next state
        /// </summary>
        public List<string> OnStateExit();

        /// <summary>
        /// 
        /// </summary>
    }
}
