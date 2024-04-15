import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import Conversation from "./Conversation";
import { Message } from "../Model";
import MessagePanel from "./MessagePanel";
import { Divider } from "@material-ui/core";
import { askQuestion } from "../lib/client";
import { getTime } from "../lib/utils";
import { startSynthesis, stopSynthesis } from "../lib/synth";

export interface IChatProps {
  conversationId: string;
  conversation: Message[];
  onConversationUpdated: (messages: Message[]) => void;
  onConversationCleared: () => void;
  onMessagePlay: (index: number) => void;
  onMessageStop: (index: number) => void;
  onFeedback: (index: number, feedback: string) => void;
}
const Chat = (props: IChatProps) => {
  const onPlay = (index: number, text: string, languageCode: string) => {
    let synthLanguage = "en-US";
    switch (languageCode.toLocaleLowerCase()) {
      case "en":
        synthLanguage = "en-US";
        break;
      case "es":
        synthLanguage = "es-ES";
        break;
      case "it":
        synthLanguage = "it-IT";
        break;
      case "de":
        synthLanguage = "de-DE";
        break;
      case "fr":
        synthLanguage = "fr-FR";
        break;
    }

    const previousPlayIndex = props.conversation.findIndex(
      (item) => item.isPlaying === true
    );
    if (previousPlayIndex >= 0) {
      stopSynthesis();
      props.onMessageStop(previousPlayIndex);
    }

    startSynthesis(
      text,
      synthLanguage,
      () => {
        props.onMessagePlay(index);
      },
      () => {
        props.onMessageStop(index);
      }
    );
    props.onMessagePlay(index);
  };

  const onStop = (index: number) => {
    stopSynthesis();
    props.onMessageStop(index);
  };

  const getAnswer = async (questionText: string, playVoice: boolean) => {
    const response = await askQuestion(props.conversationId, questionText);

    props.onConversationUpdated([
      {
        author: "Human",
        text: questionText,
        time: getTime(),
      },
      {
        author: "Bot",
        text: response.answer as string,
        time: getTime(),
        languageCode: response.questionLanguageCode,
        isPlaying: playVoice,
      },
    ]);
    if (playVoice) {
      onPlay(
        props.conversation.length - 1,
        response.answer,
        response.questionLanguageCode
      );
    }
  };

  const handleConversationCleared = () => {
    props.onConversationCleared();
  };
  const handleQuestion = (questionText: string, byVoice: boolean) => {
    props.onConversationUpdated([
      {
        author: "Human",
        text: questionText,
        time: getTime(),
      },
    ]);
    getAnswer(questionText, byVoice);
  };
  return (
    <div>
      <Grid container className="padding-16">
        <Grid item xs={12}>
          <Typography variant="h5" className="header-message">
            Welcome to your myMSC personal assistant
          </Typography>

          <Divider className="header-divider"></Divider>
        </Grid>
      </Grid>
      <Grid container className="chat-panel padding-16">
        <Conversation
          conversationHistory={props.conversation}
          onPlay={onPlay}
          onStop={onStop}
          onFeedback={props.onFeedback}
        ></Conversation>
        <Divider />
        <MessagePanel
          questionAsked={(text, byVoice) => handleQuestion(text, byVoice)}
          conversationCleared={() => handleConversationCleared()}
        ></MessagePanel>
      </Grid>
    </div>
  );
};

export default Chat;
