using Apos.Gui;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MonogameTemplate1_0.GameComponent;

namespace MonogameTemplate1_0
{
    /// <summary>
    /// Do not all code when using this class, only in the constructor. Only override when absolutely nessecary
    /// </summary>
    public abstract class Entity
    {
        public readonly long Id = Random.Shared.NextInt64();

        private List<GameComponent> components = new List<GameComponent>();

        public List<GameComponent> Components { get {  return components; } }

        public void AddComponent(GameComponent component)
        {
            components.Add(component);
        }

        public void AddComponent<T>(T component, out T comp) where T : GameComponent
        {
            components.Add(component);
            comp = (T)component;
        }

        public void RemoveComponent(Type type)
        {
            foreach (GameComponent Icomponent in components)
            {
                if (type == Icomponent.GetType())
                {
                    components.Remove(Icomponent);
                }
            }
        }

        /// <summary>
        /// DO NOT USE UNLESS YOU HAVE TO. Use the one present in base class instead
        /// </summary>
        public T GetComponent<T>() where T : GameComponent
        {
            var comp = Components.FirstOrDefault(c => c.GetType() == typeof(T));
            if(comp != null)
            {
                return comp as T;
            }
            throw new Exception($"Entity {GetType()} does not have component {typeof(T)}");
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (GameComponent Icomponent in components)
            {
                Icomponent.Tick(deltaTime);
            }
        }

        public virtual void Draw()
        {
            foreach (GameComponent Icomponent in components)
            {
                Icomponent.Draw();
            }
        }
    }

    /// <summary>
    /// Component Class for the ECF. has tick and draw, but may not be used
    /// </summary>
    public abstract class GameComponent
    {
        protected Entity parent;
        protected GameComponent(Entity parent) 
        {
            this.parent = parent;
        }

        public delegate void OnTick(float deltaTime);
        public delegate void OnDraw();

        private OnTick _onTick;

        private OnDraw _onDraw;

        /// <summary>
        /// Call before function
        /// </summary>
        public void Tick(float deltaTime)
        {
            _onTick?.Invoke(deltaTime);
            DoTick(deltaTime);
        }

        /// <summary>
        /// Call before function
        /// </summary>
        public void Draw()
        {
            _onDraw?.Invoke();
            DoDraw();
        }

        protected virtual void DoDraw() { }

        protected virtual void DoTick(float deltaTime) { }

        protected T GetParentComponent<T>() where T : GameComponent
        {
            var component = parent.Components.FirstOrDefault(c => typeof(T) == c.GetType()) ?? 
                throw new Exception($"Parent does not have component of type {typeof(T).Name}. From {GetType()}");
            return (T)component;
        }
        
        public void SetOnDraw(OnDraw onDraw)
        {
            _onDraw = onDraw;
        }
        public void SetOnTick(OnTick onTick)
        {
            _onTick = onTick;
        }
    }

    /// <summary>
    /// System Class for the ECF. Needs implementation ForEach, Validation
    /// </summary>
    public abstract class GameSystem
    {
        public void Tick(List<Entity> StateEntities, float deltaTime)
        {
            OnTickStart(deltaTime);

            List<Entity> Entities = new List<Entity>();

            foreach (var Entity in StateEntities)
            {
                if (HasControlOver(Entity))
                {
                    Entities.Add(Entity);
                }
            }

            foreach (var Entity in Entities)
            {
                ForEachEntity(Entity, Entities, deltaTime);
            }
        }

        protected virtual void OnTickStart(float deltaTime)
        {

        }

        protected static bool TryGetComponent<T>(Entity entity, out T component) where T : GameComponent
        {
            component = entity.Components.FirstOrDefault(c => typeof(T) == c.GetType()) as T;

            return component != null;
        }

        protected abstract void ForEachEntity(Entity Entity, List<Entity> AllEntity, float deltaTime);

        protected abstract bool HasControlOver(Entity Entity);
    }
}
