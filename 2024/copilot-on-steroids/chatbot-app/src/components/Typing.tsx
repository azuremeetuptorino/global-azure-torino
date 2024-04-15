import Grid from "@material-ui/core/Grid";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import { ListItemAvatar } from "@mui/material";
import BotAvatar from "./BotAvatar";

export interface TypingProps {
  message: string;
}

const Typing = (props: TypingProps) => {
  return (
    <ListItem className="left">
      <ListItemAvatar>
        <BotAvatar></BotAvatar>
      </ListItemAvatar>
      <Grid container>
        <Grid item xs={12}>
          <ListItemText
            secondary="myMSC Assistant"
            disableTypography
            className="conversation-author left"
          ></ListItemText>
        </Grid>
        <Grid item xs={12}>
          <div
            className="snippet left conversation-message"
            data-title="dot-typing"
          >
            <div>
              {props.message}
              <div className="stage">
                <div className="dot-typing"></div>
              </div>
            </div>
          </div>
        </Grid>
      </Grid>
    </ListItem>
  );
};

export default Typing;
