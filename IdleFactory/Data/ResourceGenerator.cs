namespace IdleFactory.Data
{
  public class ResourceGenerator
  {
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// Gets the time in seconds, how long it takes to genrate the <see cref="GenerationAmount"/>.
    /// </summary>
    public float GenerationTime { get; set; }

    public float ToNextGeneration { get; set; }

    public LargeInteger GenerationAmount { get; set; }
  }
}
