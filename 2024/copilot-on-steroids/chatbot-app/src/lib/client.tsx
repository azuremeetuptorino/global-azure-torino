export const askQuestion = async (
  conversationId: string,
  questionText: string
) => {
  const response = await fetch(
    "https://localhost:7144/Completion/ask",
    {
      method: "POST",
      mode: "cors", // no-cors, *cors, same-origin
      cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        conversationId: conversationId,
        questionText: questionText,
      }),
    }
  );
  return await response.json();
};
