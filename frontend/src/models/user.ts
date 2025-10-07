import type { UserRole } from "./user-role";

export interface User {
  telegramId: number;
  name: string;
  role: UserRole;
}
