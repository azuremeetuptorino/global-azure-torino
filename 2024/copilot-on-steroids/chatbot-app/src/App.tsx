import { useRef, useState } from "react";
import "./App.css";
import Chat from "./components/Chat";
import { Drawer } from "@mui/material";
import { Message } from "./Model";
import { getTime } from "./lib/utils";

function App() {
  const defaultConversation: Message[] = [
    {
      author: "Bot",
      //text: "Hello I am Emma, your myMSC virtual assistant. I can help you list and filter your existing quotes, and ask for new ones.",   
      text: "Hello I am Emma, your weather virtual assistant. ask me about the current or past weather in different cities and i will try to provide you an answer.",   
      time: getTime(),
    },
  ];

  const [chatOpen, setChatOpen] = useState(false);
  const [conversationHistory, setConversationHistory] =
    useState(defaultConversation);
  const [conversationId, setConversationId] = useState(crypto.randomUUID());
  const stateRef = useRef<Message[]>();
  stateRef.current = conversationHistory;

  const onConversationUpdated = (messages: Message[]) => {
    const newConversation = [...conversationHistory, ...messages];
    setConversationHistory(newConversation);
    stateRef.current = newConversation;
  };

  const onConversationCleared = () => {
    setConversationId(crypto.randomUUID());
    setConversationHistory(defaultConversation);
    stateRef.current = defaultConversation;
  };

  const onMessagePlay = (index: number) => {
    if (stateRef.current) {
      stateRef.current[index].isPlaying = true;
    }
    const newConversation = [...(stateRef.current as Message[])];
    setConversationHistory(newConversation);
    stateRef.current = newConversation;
  };

  const onMessageFeedback = (index: number, feedback: string) => {
    if (stateRef.current) {
      stateRef.current[index].feedback = feedback;
    }
    const newConversation = [...(stateRef.current as Message[])];
    setConversationHistory(newConversation);
    stateRef.current = newConversation;
  };

  const onMessageStop = (index: number) => {
    stateRef.current?.forEach((item) => (item.isPlaying = false));
    const newConversation = [...(stateRef.current as Message[])];
    setConversationHistory(newConversation);
    stateRef.current = newConversation;
  };

  return (
    <div>
      <Drawer
        anchor={"left"}
        open={true}
        onClose={() => setChatOpen(false)}
      >
        <Chat
          conversationId={conversationId}
          onConversationUpdated={onConversationUpdated}
          onConversationCleared={onConversationCleared}
          conversation={conversationHistory}
          onMessagePlay={onMessagePlay}
          onMessageStop={onMessageStop}
          onFeedback={onMessageFeedback}
        ></Chat>
      </Drawer>

      <li>
        <a
          className="msc-header__nav-item hide_xs header-iq-label"
          onClick={(ev) => {
            setChatOpen(true);
            ev.preventDefault();
          }}
          href="#"
        >
          <span className="icon-c-faqs iconHeaderBig helpMenu"></span>
          Help
        </a>
      </li>
    </div>
  );
}

export default App;
