import { useState } from 'react';
import styles from './ChatHeader.module.css';
import ManagementGroupButton from './ManagementGroupButton/ManagementGroupButton.jsx';
import ManagementFriendButton from './ManagementFriendButton/ManagementFriendButton.jsx';
import GroupManagementMenu from './ManagementGroupButton/GroupManagementMenu/GroupManagementMenu.jsx';
import FriendManagementMenu from './ManagementFriendButton/FriendManagementMenu/FriendManagementMenu.jsx';
import { useGroupMembersCount } from '../../../hooks/useGroupMembersCount.jsx';

export default function ChatHeader({ selectedFriend, selectedGroup, onCloseItem}) {
  const [isGroupMenuOpen, setIsGroupMenuOpen] = useState(false);
  const [isFriendMenuOpen, setIsFriendMenuOpen] = useState(false);

  const { count, loading, error } = useGroupMembersCount(selectedGroup?.id);

  const handleGroupMenuToggle = () => {
    setIsGroupMenuOpen(!isGroupMenuOpen);
    setIsFriendMenuOpen(false);
  };

  const handleFriendMenuToggle = () => {
    setIsFriendMenuOpen(!isFriendMenuOpen);
    setIsGroupMenuOpen(false);
  };

  const handleMenuClose = () => {
    setIsGroupMenuOpen(false);
    setIsFriendMenuOpen(false);
  };

  const activeItem = selectedFriend || selectedGroup;
  if (!activeItem) return null;

  return (
    <div className={styles["chat-header-container"]}>
      <div className={styles["header-content"]}>
        {selectedFriend && (
          <>
            <div className={styles["friend-info"]}>
              <p className={styles["friend-name"]}>
                {selectedFriend.firstName} {selectedFriend.lastName}
              </p>
              <p className={styles["friend-status"]}>В сети</p>
            </div>
            <ManagementFriendButton onClick={handleFriendMenuToggle} />
            
            <FriendManagementMenu 
              isOpen={isFriendMenuOpen} 
              onClose={handleMenuClose} 
              selectedItem={selectedFriend}
              onCloseItem={onCloseItem}
            />
          </>
        )}

        {selectedGroup && (
          <>
            <div className={styles["group-info"]}>
              <p className={styles["group-name"]}>{selectedGroup.name}</p>
              <span className={styles["members-count"]}>
                {count} {getMembersText(count)}
              </span>
            </div>
            <ManagementGroupButton onClick={handleGroupMenuToggle} />
            
            <GroupManagementMenu
              isOpen={isGroupMenuOpen} 
              onClose={handleMenuClose} 
              selectedItem={selectedGroup}
              onCloseItem={onCloseItem}
            />
          </>
        )}
      </div>
    </div>
  );
}

const getMembersText = (count) => {
    if (count % 10 === 1 && count % 100 !== 11) return 'участник';
    if ([2, 3, 4].includes(count % 10) && ![12, 13, 14].includes(count % 100)) 
        return 'участника';
    return 'участников';
};
