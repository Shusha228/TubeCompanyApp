import type { UserRole } from "./user-role";

export interface User {
  name: string;
  role: UserRole;
}
