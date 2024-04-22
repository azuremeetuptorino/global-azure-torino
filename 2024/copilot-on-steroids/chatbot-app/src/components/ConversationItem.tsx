import Grid from "@material-ui/core/Grid";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import { IconButton, ListItemAvatar } from "@mui/material";
import BotAvatar from "./BotAvatar";
import ThumbUpIcon from "@mui/icons-material/ThumbUp";
import ThumbDownIcon from "@mui/icons-material/ThumbDown";
import PlayCircleFilledWhiteIcon from "@mui/icons-material/PlayCircleFilledWhite";
import StopCircleIcon from "@mui/icons-material/StopCircle";
import { useState } from "react";
export interface ConversationItemProps {
  index: number;
  text: string;
  time: string;
  author: string;
  languageCode?: string;
  isPlaying?: boolean;
  feedback?: string;
  onPlay: (index: number, text: string, languageCode: string) => void;
  onStop: (index: number) => void;
  onFeedback: (index: number, feedback: string) => void;
}

const ConversationItem = (props: ConversationItemProps) => {
  const htmlMessage = props.text.replaceAll("\n", "<br />");
  return (
    <ListItem
      key={props.index}
      className={props.author === "Bot" ? "left" : "right"}
    >
      {props.author === "Bot" ? (
        <ListItemAvatar>
          <BotAvatar></BotAvatar>
        </ListItemAvatar>
      ) : null}
      <Grid container>
        <Grid item xs={12}>
          {props.author === "Bot" ? (
            <ListItemText
              secondary="myMSC Assistant"
              disableTypography
              className={
                "conversation-author " +
                (props.author === "Bot" ? "left" : "right")
              }
            ></ListItemText>
          ) : null}
          <ListItemText
            secondary={props.time}
            disableTypography
            className={
              "conversation-time " + (props.author === "Bot" ? "left" : "right")
            }
          ></ListItemText>
        </Grid>

        <Grid item xs={12}>
          <div
            className={
              "conversation-message " +
              (props.author === "Bot" ? "left" : "right")
            }
          >
            <div dangerouslySetInnerHTML={{ __html: htmlMessage }}></div>
            {props.author === "Bot" ? (
              <div>
                <div className="play">
                  {!props.isPlaying ? (
                    <IconButton
                      aria-label="play"
                      onClick={() => {
                        props.onPlay(
                          props.index,
                          props.text,
                          props.languageCode ?? ""
                        );
                      }}
                    >
                      <PlayCircleFilledWhiteIcon />
                    </IconButton>
                  ) : null}
                  {props.isPlaying ? (
                    <IconButton
                      aria-label="pause"
                      onClick={() => {
                        props.onStop(props.index);
                      }}
                    >
                      <StopCircleIcon></StopCircleIcon>
                    </IconButton>
                  ) : null}
                </div>
                {props.index > 0 ? (
                  <div className="feedback">
                    {props.feedback !== "like" ? (
                      <IconButton
                        onClick={() => props.onFeedback(props.index, "dislike")}
                      >
                        <ThumbDownIcon
                          fontSize="small"
                          className="extraSmallIcon"
                        ></ThumbDownIcon>
                      </IconButton>
                    ) : null}
                    &nbsp;&nbsp;
                    {props.feedback !== "dislike" ? (
                      <IconButton
                        onClick={() => props.onFeedback(props.index, "like")}
                      >
                        <ThumbUpIcon
                          fontSize="small"
                          className="extraSmallIcon"
                        ></ThumbUpIcon>
                      </IconButton>
                    ) : null}
                  </div>
                ) : null}
              </div>
            ) : null}
          </div>
        </Grid>
        <Grid item xs={12}></Grid>
      </Grid>
    </ListItem>
  );
};

export default ConversationItem;
