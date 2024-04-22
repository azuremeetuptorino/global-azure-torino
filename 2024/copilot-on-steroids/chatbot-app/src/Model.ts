export interface Message {
  author: string;
  text: string;
  time: string;
  languageCode?: string;
  isPlaying?: boolean;
  feedback?: string;
}
