import { IComment } from './IComment';
import { IUser } from './IUser';

export interface IPost {
  id: number;
  content: string;
  createdAt: string;
  image: string;
  userImage: File;
  likes: number;
  liked: boolean;
  comments: IComment[];
  imageUrl: string;
  user: IUser;
  likedUserIds: string[];
}
