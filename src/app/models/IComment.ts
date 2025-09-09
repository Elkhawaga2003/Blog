import { IUser } from './IUser';

export interface IComment {
  id: number;
  content: string;
  user: IUser;
  postId: number;
}
