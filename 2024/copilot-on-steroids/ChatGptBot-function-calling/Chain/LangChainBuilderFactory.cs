using ChatGptBot.Ioc;

namespace ChatGptBot.Chain;
public interface ILangChainBuilderFactory
{
    ILangChainBuilder Create();
}
public class LangChainBuilderFactory : ILangChainBuilderFactory, ISingletonScope
{
    public ILangChainBuilder Create() { return  new ChainBuilder(); }
}