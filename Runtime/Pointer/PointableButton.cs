using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VED.Utilities
{
    public enum ActionType
    {
        PRESS,
        UNPRESS
    }

    public class PointableButton : Pointable
    {
        public enum State
        {
            UNPOINTED,
            POINTED  ,
            PRESSED  ,
            DISABLED
        }

        private StateManager<StatePointableButton> _stateManager = null;
    
        [SerializeField, Space(10)] private List<PointableButtonAnimationTransformScale    > _animationTransformScales     = new List<PointableButtonAnimationTransformScale    > { };
        [SerializeField, Space(10)] private List<PointableButtonAnimationTransformZPosition> _animationTransformZPositions = new List<PointableButtonAnimationTransformZPosition> { };
        [SerializeField, Space(10)] private List<PointableButtonAnimationGraphicColor      > _animationGraphicColors       = new List<PointableButtonAnimationGraphicColor      > { };
        [SerializeField, Space(10)] private List<PointableButtonAnimationGraphicAlpha      > _animationGraphicAlphas       = new List<PointableButtonAnimationGraphicAlpha      > { };

        [SerializeField, Space(10)] public UnityEvent Action = null;
        [SerializeField] public ActionType ActionType = ActionType.UNPRESS;

        public StateManager<StatePointableButton>               StateManager                 => _stateManager;
        public List<PointableButtonAnimationTransformScale    > AnimationTransformScales     => _animationTransformScales;
        public List<PointableButtonAnimationTransformZPosition> AnimationTransformZPositions => _animationTransformZPositions;
        public List<PointableButtonAnimationGraphicColor      > AnimationGraphicColors       => _animationGraphicColors;
        public List<PointableButtonAnimationGraphicAlpha      > AnimationGraphicAlphas       => _animationGraphicAlphas;

        public override void Init()
        {
            base.Init();

            _stateManager = new StateManager<StatePointableButton>();

            StatePointableButtonUnpointed pointableButtonStateUnpointed = new();
            _stateManager.Push(pointableButtonStateUnpointed);

            pointableButtonStateUnpointed.Init(this);
        }

        public override void Tick()
        {
            _stateManager.Tick();
        }

        public override void Point(Pointer pointer, Vector3 position)
        {
            base.Point(pointer, position);

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Point();
        }

        public override void Unpoint(Pointer pointer)
        {
            base.Unpoint(pointer);

            if (Pointed)
                return;

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Unpoint();
        }

        public override void Press(Pointer pointer, Vector3 position) 
        {
            base.Press(pointer, position);

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Press();

            if (ActionType == ActionType.PRESS)
                Action?.Invoke();
        }

        public override void Unpress(Pointer pointer, bool cancel)
        {
            base.Unpress(pointer, cancel);

            if (Pressed)
                return;

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Unpress();

            if (!cancel && ActionType == ActionType.UNPRESS)
                Action?.Invoke();
        }

        public override void Enable()
        {
            base.Enable();

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Enable();
        }

        public override void Disable()
        {
            base.Disable();

            if (!_stateManager.TryPeek(out StatePointableButton pointableButtonState))
                return;

            pointableButtonState.Disable();
        }
    }

    public abstract class StatePointableButton : State
    {
        protected PointableButton _pointableButton = null;
    
        public virtual void Init(PointableButton pointableButton)
        {
            _pointableButton = pointableButton;
        }
    
        protected void TickAnimatables(PointableButton.State state)
        {
            for (int i = 0; i < _pointableButton.AnimationTransformScales.Count; i++)
                _pointableButton.AnimationTransformScales[i].Tick(state);

            for (int i = 0; i < _pointableButton.AnimationTransformZPositions.Count; i++)
                _pointableButton.AnimationTransformZPositions[i].Tick(state);

            for (int i = 0; i < _pointableButton.AnimationGraphicColors.Count; i++)
                _pointableButton.AnimationGraphicColors[i].Tick(state);

            for (int i = 0; i < _pointableButton.AnimationGraphicAlphas.Count; i++)
                _pointableButton.AnimationGraphicAlphas[i].Tick(state);
        }
    
        public virtual void Point  () { }
        public virtual void Unpoint() { }
        public virtual void Press  () { }
        public virtual void Unpress() { }
        public virtual void Enable () { }
        public virtual void Disable() { }
    }
    
    public class StatePointableButtonUnpointed : StatePointableButton
    {
        public override void Tick()
        {
            TickAnimatables(PointableButton.State.UNPOINTED);
        }
    
        public override void Point()
        {
            StatePointableButtonPointed pointableButtonStatePointed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStatePointed);

            pointableButtonStatePointed.Init(_pointableButton);
        }
    
        public override void Disable()
        {
            StatePointableButtonDisabled pointableButtonStateDisabled = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateDisabled);

            pointableButtonStateDisabled.Init(_pointableButton);
        }
    }
    
    public class StatePointableButtonPointed : StatePointableButton
    {
        public override void Tick()
        {
            TickAnimatables(PointableButton.State.POINTED);
        }
    
        public override void Unpoint()
        {
            StatePointableButtonUnpointed pointableButtonStateUnpointed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateUnpointed);

            pointableButtonStateUnpointed.Init(_pointableButton);
        }
    
        public override void Press()
        {
            StatePointableButtonPressed pointableButtonStatePressed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStatePressed);

            pointableButtonStatePressed.Init(_pointableButton);
        }
    
        public override void Disable()
        {
            StatePointableButtonDisabled pointableButtonStateDisabled = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateDisabled);

            pointableButtonStateDisabled.Init(_pointableButton);
        }
    }
    
    public class StatePointableButtonPressed : StatePointableButton
    {
        public override void Tick()
        {
            TickAnimatables(PointableButton.State.PRESSED);
        }
    
        public override void Unpoint()
        {
            if (_pointableButton.Pointed)
                return;

            StatePointableButtonUnpointed pointableButtonStateUnpointed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateUnpointed);

            pointableButtonStateUnpointed.Init(_pointableButton);
        }
    
        public override void Unpress()
        {
            StatePointableButtonPointed pointableButtonStatePointed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStatePointed);

            pointableButtonStatePointed.Init(_pointableButton);
        }
    
        public override void Disable()
        {
            StatePointableButtonDisabled pointableButtonStateDisabled = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateDisabled);

            pointableButtonStateDisabled.Init(_pointableButton);
        }
    }
    
    public class StatePointableButtonDisabled : StatePointableButton
    {
        public override void Tick()
        {
            TickAnimatables(PointableButton.State.DISABLED);
        }
    
        public override void Enable()
        {
            StatePointableButtonUnpointed pointableButtonStateUnpointed = new();

            _pointableButton.StateManager.Pop();
            _pointableButton.StateManager.Push(pointableButtonStateUnpointed);

            pointableButtonStateUnpointed.Init(_pointableButton);
        }
    }
}