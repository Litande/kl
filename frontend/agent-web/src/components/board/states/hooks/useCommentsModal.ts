import useToggle from "hooks/useToggle";

const useCommentsModal = () => {
  const [isAllCommentsModalShown, toggleIsAllCommentsModalShown] = useToggle(false);
  const [isNewCommentsModalShown, toggleIsNewCommentsModalShown] = useToggle(false);

  const handleAddCommentButtonClick = () => {
    toggleIsNewCommentsModalShown();
    toggleIsAllCommentsModalShown();
  };

  const handleBackButtonClick = () => {
    toggleIsNewCommentsModalShown();
    toggleIsAllCommentsModalShown();
  };

  const handleCloseModal = () => {
    isNewCommentsModalShown && toggleIsNewCommentsModalShown();
    isAllCommentsModalShown && toggleIsAllCommentsModalShown();
  };

  return {
    isAllCommentsModalShown,
    isNewCommentsModalShown,
    handleAddCommentButtonClick,
    handleBackButtonClick,
    toggleIsAllCommentsModalShown,
    toggleIsNewCommentsModalShown,
    handleCloseModal,
  };
};

export default useCommentsModal;
