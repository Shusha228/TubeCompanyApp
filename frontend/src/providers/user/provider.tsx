import { UserRole } from "@/models/user-role";
import WebApp from "@twa-dev/sdk";
import type { JSX } from "react";
import { UserContext } from "./context";

export const UserProvider = ({ children }: { children: JSX.Element }) => {
  const user = WebApp.initDataUnsafe.user;

  return (
    <UserContext.Provider
      value={{
        telegramId: user !== undefined ? user.id : 1,
        name: [user?.first_name, user?.last_name]
          .filter((el) => el !== undefined)
          .join(" "),
        role: UserRole.User,
        photo: user?.photo_url,
      }}
    >
      {children}
    </UserContext.Provider>
  );
};
