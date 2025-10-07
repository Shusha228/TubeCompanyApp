import type { User } from "@/models/user";
import { createContext } from "react";

export const UserContext = createContext({} as User);