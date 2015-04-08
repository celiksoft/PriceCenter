using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace myBluetoothProject.Messenger
{
    public class Messenger<TMsg>
    {
        #region Static region

        public readonly static Messenger<TMsg> Default = new Messenger<TMsg>();

        #endregion

        #region Events & Event methods

        #endregion

        #region Fields

        private Dictionary<TMsg, List<Delegate>> _handlers;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        // Singleton
        private Messenger()
        {
            _handlers = new Dictionary<TMsg, List<Delegate>>();
        }

        #endregion

        #region Methods

        private void AddHandler(TMsg message, Delegate handler)
        {
            if (handler == null)
                throw new ArgumentNullException("Handler cannot; be null");

            if (_handlers.ContainsKey(message) == false)
                _handlers.Add(message, new List<Delegate>());

            _handlers[message].Add(handler);
        }

        public void AddHandler(TMsg message, Action handler)
        {
            AddHandler(message, (Delegate)handler);
        }

        public void AddHandler<T>(TMsg message, Action<T> handler)
        {
            AddHandler(message, (Delegate)handler);
        }

        public void AddHandler<T, U>(TMsg message, Action<T, U> handler)
        {
            AddHandler(message, (Delegate)handler);
        }

        private void RemoveHandler(TMsg message, Delegate handler)
        {
            if (_handlers.ContainsKey(message) == false)
                return; // message does not exist;

            _handlers[message].Remove(handler);
        }

        public void RemoveHandler(TMsg message, Action handler)
        {
            RemoveHandler(message, (Delegate)handler);
        }

        public void RemoveHandler<T>(TMsg message, Action<T> handler)
        {
            RemoveHandler(message, (Delegate)handler);
        }

        public void RemoveHandler<T, U>(TMsg message, Action<T, U> handler)
        {
            RemoveHandler(message, (Delegate)handler);
        }

        public void Publish(TMsg message, params object[] prms)
        {
            if (_handlers.ContainsKey(message) == false)
                return; // no registration no publish

            _handlers[message].ForEach(handler => handler.DynamicInvoke(prms));
        }

        #endregion
    }
}
