import { AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Modal } from "@/modals/models/modal";
import { UserRole } from "@/models/user-role";
import { useActiveModal } from "@/providers/active-modal";
import { useUser } from "@/providers/user";
import { Avatar } from "@radix-ui/react-avatar";
import { HelpCircleIcon, History, LayoutGrid, Plus } from "lucide-react";

export const ProfilePanel = () => {
  const user = useUser();
  const { showModal } = useActiveModal();

  const fallback = user.name
    .split(" ")
    .slice(0, 2)
    .map((e) => e[0])
    .join("");

  return (
    <div className="flex flex-col w-full bg-[#F3F3F3] h-auto">
      <div className="flex pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
        <div className="flex w-full gap-4 items-center px-4">
          <Avatar className="w-24 h-24">
            <AvatarImage src={user.photo}></AvatarImage>
            <AvatarFallback className="text-lg">{fallback}</AvatarFallback>
          </Avatar>
          <div className="w-auto max-w-full">
            <p className="text-black font-medium text-md">{user.role}</p>
            <h1 className="text-black font-bold text-lg">{user.name}</h1>
          </div>
        </div>
      </div>
      <div className="w-full h-[10px]"></div>
      <div className="pb-18 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
        <div className="w-full flex flex-col px-4 gap-2">
          {user.role == UserRole.User && (
            <>
              <Button variant="outline" size="lg">
                <History className="mr-1.5" color="#EC6608" />
                <div className="h-full bg-[#DEDEDE] w-[1px]"></div>
                <span className="text-center w-full">История заказов</span>
              </Button>

              <Button variant="outline" size="lg">
                <HelpCircleIcon className="mr-1.5" color="#EC6608" />
                <div className="h-full bg-[#DEDEDE] w-[1px]"></div>
                <span className="text-center w-full">
                  Техническая поддержка
                </span>
              </Button>
            </>
          )}
          {user.role == UserRole.Admin && (
            <>
              <Button
                variant="outline"
                size="lg"
                onClick={() => showModal(Modal.AllItems)}
              >
                <LayoutGrid className="mr-1.5" color="#EC6608" />
                <div className="h-full bg-[#DEDEDE] w-[1px]"></div>
                <span className="text-center w-full">Все товары</span>
              </Button>

              <Button
                variant="outline"
                size="lg"
                onClick={() => showModal(Modal.NewItem)}
              >
                <Plus className="mr-1.5" color="#EC6608" />
                <div className="h-full bg-[#DEDEDE] w-[1px]"></div>
                <span className="text-center w-full">Добавить товар</span>
              </Button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};
