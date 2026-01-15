namespace Fletch.Engine.Model
{
    public enum SystemExecutionOrder
    {
        Input = 0,

        PreUpdate = 100,

        Update = 200,

        PostUpdate = 300,

        Simulation = 400,

        PostSimulation = 500,

        RenderPrep = 600,

        Rendering = 700,

        EndOfFrame = 800
    }
}
