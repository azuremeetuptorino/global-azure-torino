import { makeStyles } from "@material-ui/core";

export const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
  chatSection: {
    width: "100%",
    height: "80vh",
  },
  headBG: {
    backgroundColor: "#e0e0e0",
  },
  borderRight500: {
    borderRight: "1px solid #e0e0e0",
  },
  messageArea: {
    maxHeight: "80vh",
    overflowY: "auto",
    marginBottom: "16px",
  },
});
