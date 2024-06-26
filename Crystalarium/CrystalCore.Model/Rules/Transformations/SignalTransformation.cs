﻿using CrystalCore.Model.Communication;
using CrystalCore.Model.Language;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using System.Collections.Generic;

namespace CrystalCore.Model.Rules.Transformations
{
    public class SignalTransformation : ITransformation
    {

        public bool ForrbiddenInDefaultState
        {
            get
            {
                return false;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return false;
            }
        }

        private PortDescriptor[] ports;
        private Expression value;

        public SignalTransformation(Expression value, params PortDescriptor[] ports)
        {


            // give all ports this value.
            this.ports = ports;
            this.value = value;


        }

        public SignalTransformation(int value, params PortDescriptor[] ports)
        {
            this.value = new IntOperand(value);
            this.ports = ports;
        }

        public Transform CreateTransform(Agent a)
        {

            int val = (int)value.Resolve(a).Value;

            return (a) =>
            {

                List<PortTransmission> toTransmit = new();

                for (int i = 0; i < ports.Length; i++)
                {
                    toTransmit.Add(new(val, ports[i]));
                }

                a.TransmitOn(toTransmit.ToArray());
            };




        }

        public void Validate(AgentType at)
        {

            foreach (PortDescriptor port in ports)
            {
                if (!at.IsDescriptorValid(port))
                {
                    throw new InitializationFailedException(
                        "Signal Transformation: Port ID: " + port.ID + " is not valid for AgentType '" + at.Name + "'.");
                }
            }

            try
            {
                value.Initialize();
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("A Signal Transformation was invalid:" + MiscUtil.Indent(e.Message));
            }

            if (value.ReturnType != TokenType.integer)
            {
                throw new InitializationFailedException("Signal Transformation: Value to transmit must be an integer.");
            }

        }
    }
}
