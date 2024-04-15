using ChatGptBot.Chain.Dto;

namespace ChatGptBot.Chain;

public interface ILangChainBrick
{
    void SetNext(ILangChainBrick langChainBrickBase);
    
    public ILangChainBrick? Next { get; }

    public abstract Task<Answer> Ask(Question question);
}

public abstract class LangChainBrickBase : ILangChainBrick
{
    public void SetNext(ILangChainBrick langChainBrickBase)
    {
        Next = langChainBrickBase;
    }

    public ILangChainBrick? Next { get; private set; }

    public abstract Task<Answer> Ask(Question question);

}