import { getURL } from "@/api";
import type { User } from "@/models/user";
import { UserRole } from "@/models/user-role";
import WebApp from "@twa-dev/sdk";
import { useEffect, useState, type JSX } from "react";
import { UserContext } from "./context";

export const UserProvider = ({ children }: { children: JSX.Element }) => {
  const _user = WebApp.initDataUnsafe.user;
  const [user] = useState<User>({
    telegramId: _user !== undefined ? _user.id : 2000,
    name: [_user?.first_name, _user?.last_name]
      .filter((el) => el !== undefined)
      .join(" "),
    role: UserRole.User,
    photo: _user?.photo_url,
  });

  useEffect(() => {
    if (user !== undefined) {
      fetch(getURL(`TelegramUsers/${user.telegramId}`)).then((el) => {
        if (el.status !== 200) {
          fetch(getURL(`TelegramUsers/`), {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              telegramUserId: user?.telegramId,
              firstName: _user?.first_name,
              lastName: _user?.last_name,
              username: _user?.username,
            }),
          });
        }
      });
    }
  }, [_user]);

  return <UserContext.Provider value={user}>{children}</UserContext.Provider>;
};
