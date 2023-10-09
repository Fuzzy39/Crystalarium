namespace CrystalCore.Model.Rules.Transformations
{
    /// <summary>
    /// A way an agent can change itself or it's environment
    /// </summary>
    public interface ITransformation
    {
        // Transformations should set this property. default states cannot have transformations that change the agent.
        internal protected bool ForrbiddenInDefaultState { get; }
        internal protected bool MustBeLast { get; }

        internal void Validate(AgentType at);

        internal Transform CreateTransform(Agent a);



    }

    public delegate void Transform(Agent a);

}
