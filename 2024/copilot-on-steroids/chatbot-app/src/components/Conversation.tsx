import Grid from "@material-ui/core/Grid";
import List from "@material-ui/core/List";
import { Message } from "../Model";
import ConversationItem from "./ConversationItem";
import { useStyles } from "../lib/makeStyle";
import { useEffect, useRef } from "react";
import Typing from "./Typing";

interface ConversationProps {
  conversationHistory: Message[];
  onPlay: (index: number, text: string, languageCode: string) => void;
  onStop: (index: number) => void;
  onFeedback: (index: number, feedback: string) => void;
}

const Conversation = (props: ConversationProps) => {
  const classes = useStyles();
  const scrollRef = useRef(null);

  useEffect(() => {
    if (scrollRef.current) {
      (scrollRef.current as any).scrollIntoView({
        behaviour: "smooth",
        block: "end",
      });
    }
  }, [props.conversationHistory]);

  return (
    <Grid item xs={12}>
      <List className={classes.messageArea}>
        {props.conversationHistory.map((item, index) => {
          return (
            <ConversationItem
              key={index}
              author={item.author}
              index={index}
              text={item.text}
              time={item.time}
              isPlaying={item.isPlaying}
              languageCode={item.languageCode}
              feedback={item.feedback}
              onPlay={(index, text, languageCode) =>
                props.onPlay(index, text, languageCode)
              }
              onStop={(index) => props.onStop(index)}
              onFeedback={props.onFeedback}
            ></ConversationItem>
          );
        })}
        {props.conversationHistory[props.conversationHistory.length - 1]
          .author === "Human" ? (
          <Typing message="Bot is replying"></Typing>
        ) : null}
        <li ref={scrollRef}>&nbsp;</li>
      </List>
    </Grid>
  );
};

export default Conversation;
