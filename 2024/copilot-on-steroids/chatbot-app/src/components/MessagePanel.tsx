import { Fab, TextField } from "@material-ui/core";
import { CircularProgress, Stack } from "@mui/material";
import { useState } from "react";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import SendIcon from "@mui/icons-material/Send";
import MicIcon from "@mui/icons-material/Mic";
import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";
import { doRecognizeOnceAsync } from "../lib/speech";
import { Box } from "@mui/system";
export interface MessagePanelProps {
  questionAsked: (text: string, byVoice: boolean) => any;
  conversationCleared: () => any;
}

const MessagePanel = (props: MessagePanelProps) => {
  const [questionText, setQuestionText] = useState("");
  const [isSpeechToTextInProgress, setIsSpeechToTextInProgress] =
    useState(false);
  const handleQuestionChange = (e: any) => {
    setQuestionText(e.target.value);
  };

  const audioTranslatedCallback = (result: any) => {
    if (!result.privText) {
      return;
    }
    setIsSpeechToTextInProgress(false);
    onQuestion(result.privText, true);
  };

  const testAudio = () => {
    setIsSpeechToTextInProgress(true);

    // FOR VDI testing
    const debug = false;
    if (debug) {
      var filePicker = document.getElementById("filePicker") as any;
      filePicker?.addEventListener("change", function () {
        const audioFile = filePicker?.files ? filePicker?.files[0] : null;
        doRecognizeOnceAsync(audioFile, audioTranslatedCallback, true);
      });
      filePicker.click();
    } else {
      doRecognizeOnceAsync("", audioTranslatedCallback, debug);
    }
  };

  const onQuestion = (questionText: string, byVoice: boolean) => {
    props.questionAsked(questionText, byVoice);
    setQuestionText("");
  };

  return (
    <Stack
      spacing={2}
      direction="row"
      justifyContent="space-between"
      className="w-100"
    >
      <input type="file" id="filePicker" accept=".wav" className="audioInput" />
      <Box sx={{ position: "relative" }}>
        <Fab
          aria-label="like"
          className="reset"
          onClick={() => {
            testAudio();
          }}
        >
          <MicIcon />
        </Fab>
        {isSpeechToTextInProgress ? (
          <CircularProgress
            size={48}
            sx={{
              position: "absolute",
              top: 0,
              left: 0,
              zIndex: 1,
            }}
          />
        ) : null}
      </Box>
      <Fab
        aria-label="like"
        className="reset"
        onClick={() => {
          props.conversationCleared();
        }}
      >
        <RestartAltIcon />
      </Fab>
      <TextField
        id="outlined-basic-email"
        label="Ask me about your quotes ..."
        className="message-box"
        variant="outlined"
        autoComplete="off"
        value={questionText}
        onChange={handleQuestionChange}
        onKeyUp={(ev: any) => {
          if (ev.key === "Enter") {
            onQuestion(ev.target.value, false);
          }
        }}
      />
      <Fab
        className="send"
        aria-label="like"
        onClick={async (ev: any) => {
          onQuestion(questionText, false);
        }}
      >
        <SendIcon />
      </Fab>
    </Stack>
  );
};

export default MessagePanel;
