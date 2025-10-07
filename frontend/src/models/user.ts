import type { UserRole } from "./user-role";

export interface User {
  photo?: string;
  telegramId: number;
  name: string;
  role: UserRole;
  inn?: string;
}
